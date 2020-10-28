import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-reset-form',
  templateUrl: './reset-form.component.html',
  styleUrls: ['./reset-form.component.sass']
})
export class ResetFormComponent implements OnInit, OnDestroy {

  public token: string;
  public hidePass = true;
  public hidePassRe = true;
  public password: string;
  public passwordRe: string;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthenticationService
    ) { }

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.unsubscribe$))
    .subscribe(params => {
      if (!params.token) {
        this.router.navigate(['/']);
      }
      this.token = params.token;
    });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public reset(): void {
    this.authService.updatePassword(this.password, this.token)
        .pipe(takeUntil(this.unsubscribe$)).subscribe(
          (resp) => {
            console.log(resp);
            //this.snackBarService.showUsualMessage('Successfully updated');
            this.router.navigate(['/']);
          },
          (error) => console.log(error)//this.snackBarService.showErrorMessage(error)
        );
  }

}
