import { Component, OnDestroy, OnInit, ValueSansProvider } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { RolesEnum } from 'src/app/models/common/role-enum';
import { User } from 'src/app/models/user';
import { AuthenticationService } from 'src/app/services/auth.service';
import { EventService } from 'src/app/services/event.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.sass']
})
export class UserProfileComponent implements OnInit, OnDestroy {

  public currentUser: User;
  public hidePass = true;
  public hidePassRe = true;
  public password: string;
  public passwordRe: string;
  public nameWait = false;
  public toolMsg = 'Изменить может только администратор';
  private unsubscribe$ = new Subject<void>();

  constructor(
    private authService: AuthenticationService,
    private eventService: EventService,
    private userService: UserService,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.getUser();
    this.eventService.userChangedEvent$
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe((user) => (this.currentUser = user ? this.userService.copyUser(user) : undefined));
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private getUser(): ValueSansProvider {
    if (!this.authService.areTokensExist()) {
        return;
    }
    this.authService
        .getUser()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((user) => {
          this.currentUser = this.userService.copyUser(user);
          if (user.role === RolesEnum.Admin) {
            this.toolMsg = 'Нельзя изменить для администратора';
          }
          });
  }

  public save(): void {
    console.log(this.currentUser);
    this.nameWait = true;
    this.userService.updateUser(this.currentUser)
    .pipe(takeUntil(this.unsubscribe$)).subscribe(
            () => {
                this.authService.setUser(this.currentUser);
                //this.snackBarService.showUsualMessage('Successfully updated');
                this.nameWait = false;
            },
            (error) => {
                //this.snackBarService.showErrorMessage(error);
                console.log(error);
                this.nameWait = false;
            }
        );
  }

  public editPassword(): void {
    const email = this.currentUser.email;
    this.authService.editPassword(this.password)
    .pipe(takeUntil(this.unsubscribe$)).subscribe(
      () => {
        this.authService.logout();
        this.authService
        .login({ email, password: this.password })
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(() => this.router.navigate(['/profile']), (error) => console.log(error));
      },
      (error) => console.log(error)
    );
  }
}
