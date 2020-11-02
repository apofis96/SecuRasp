import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/services/auth.service';
import { NotificationService } from 'src/app/services/notification.service';
import { ResetDialogComponent } from '../reset-dialog/reset-dialog.component';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.sass']
})
export class LoginFormComponent implements OnInit, OnDestroy {
  public hidePass = true;
  public password: string;
  public email: string;
  private unsubscribe$ = new Subject<void>();
  public isLoading = false;

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    public dialog: MatDialog,
    private notify: NotificationService
  ) { }

  ngOnInit(): void {
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public signIn(): void {
    this.isLoading = true;
    this.authService
        .login({ email: this.email, password: this.password })
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(() => this.router.navigate(['/']),
        (error) => {
            this.isLoading = false;
            this.notify.showError(error);
        });
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ResetDialogComponent, {
      width: '40%'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.isLoading = true;
        this.authService
            .reset(result, window.location.host)
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe((response) => {
              this.isLoading = false;
              this.notify.showNotification('Ссылка для сброса отправлена Вам на почту.');
            },
            (error) => {
              this.isLoading = false;
              this.notify.showError(error);
            });
      }
    });
  }

}
