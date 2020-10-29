import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/services/auth.service';
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

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public signIn(): void {
    this.authService
        .login({ email: this.email, password: this.password })
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(() => this.router.navigate(['/']), (error) => console.log(error));
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ResetDialogComponent, {
      width: '40%'
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
      if (result) {
        this.authService
            .reset(result, window.location.host)
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe((response) => console.log(response), (error) => console.log(error));
      }
    });
  }

}
