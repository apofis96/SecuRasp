import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-registration-form',
  templateUrl: './registration-form.component.html',
  styleUrls: ['./registration-form.component.sass']
})
export class RegistrationFormComponent implements OnInit, OnDestroy {

  public hidePass = true;
  public hidePassRe = true;
  public password: string;
  public email: string;
  public name: string;
  public isLoading = true;
  public isAdminExist: boolean;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private authService: AuthenticationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.isAdminExist().pipe(takeUntil(this.unsubscribe$))
    .subscribe(
      (response) => {
        this.isAdminExist = response.body;
        this.isLoading = false;
      },
      (error) => console.log(error));
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public signUp(): void {
    this.authService.register({ userName: this.name, password: this.password, email: this.email})
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((response) =>
    {
      console.log(response);
      if (this.isAdminExist) {
        this.router.navigate(['/']);
      }
    },
    (error) => console.log(error));
  }
}
