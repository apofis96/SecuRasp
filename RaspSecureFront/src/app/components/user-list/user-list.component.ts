import { AfterViewInit, Component, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { merge, of as observableOf, Subject } from 'rxjs';
import { catchError, map, startWith, switchMap, takeUntil } from 'rxjs/operators';
import { EditDialogData } from 'src/app/models/common/edit-dialog';
import { EditEnum } from 'src/app/models/common/edit-dialog-enum';
import { User } from 'src/app/models/user';
import { AuthenticationService } from 'src/app/services/auth.service';
import { NotificationService } from 'src/app/services/notification.service';
import { UserService } from 'src/app/services/user.service';
import { EditDialogComponent } from '../common/edit-dialog/edit-dialog.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.sass']
})
export class UserListComponent implements OnInit, AfterViewInit, OnDestroy {
  public displayedColumns: string[] = ['id', 'email', 'name', 'role', 'created'];
  public data: User[] = [];
  public currentUser: User;
  public unactive = false;
  public toggleText = 'Показать Неактивированных';
  public resultsLength = 0;
  public isLoadingResults = true;
  public isLoadError = false;
  public widthCont = 400;
  private unsubscribe$ = new Subject<void>();
  private toggleEvent = new EventEmitter();


  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTable) table: MatTable<any>;

  constructor(
    private userService: UserService,
    private authService: AuthenticationService,
    public dialog: MatDialog,
    private notify: NotificationService
    ) {}

  ngOnInit() {
      if (!this.authService.areTokensExist()) {
          return;
      }
      this.authService
          .getUser()
          .pipe(takeUntil(this.unsubscribe$))
          .subscribe((user) => {
              this.currentUser = this.userService.copyUser(user);
            },
            (error) => (this.notify.showError(error)));
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngAfterViewInit() {
    this.paginator._intl.itemsPerPageLabel = 'Елементов на странице';

    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page, this.toggleEvent)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          this.widthCont = 400;
          return this.userService.getUsers(this.sort.direction, this.paginator.pageIndex, this.paginator.pageSize, this.unactive);
        }),
        map(data => {
          this.isLoadingResults = false;
          this.isLoadError = false;
          this.resultsLength = data.body.items;
          this.widthCont = 0;
          return data.body.users;
        }),
        catchError((error) => {
          this.notify.showError(error);
          this.isLoadingResults = false;
          this.isLoadError = true;
          return observableOf([]);
        })
      ).subscribe(data => this.data = data);
  }

  openDialog(type: EditEnum, value: any, user: User): void {
    const dialogRef = this.dialog.open(EditDialogComponent, {
      width: '35%',
      data: {type, value} as EditDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log(result);
        const newUser = this.userService.copyUser(user);
        switch (type) {
          case EditEnum.Email:
            newUser.email = result;
            break;
          case EditEnum.Name:
            newUser.userName = result;
            break;
          case EditEnum.Role:
            newUser.role = Number(result as string);
            break;
        }
        this.isLoadingResults = true;
        this.userService.adminUpdateUser(newUser).pipe(takeUntil(this.unsubscribe$)).subscribe(
          () => {
            this.isLoadingResults = false;
            this.data[this.data.indexOf(user)] = newUser;
            this.table.renderRows();
            this.notify.showNotification('Значение обновлено.');
          },
          (error) => {
              console.log(error);
              this.notify.showError(error);
              this.isLoadingResults = false;
          }
      );
      }
    });
  }

  public onToggleClick(): void {
    this.unactive = !this.unactive;
    if (this.unactive) {
      this.toggleText = 'Показать Всех';
    }
    else {
      this.toggleText = 'Показать Неактивированных';
    }
    this.paginator.pageIndex = 0;
    this.toggleEvent.emit();
  }
}
