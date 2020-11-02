import { Component, OnDestroy, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { RolesEnum } from 'src/app/models/common/role-enum';
import { AuthenticationService } from 'src/app/services/auth.service';
import { NotificationService } from 'src/app/services/notification.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-unactivated',
  templateUrl: './unactivated.component.html',
  styleUrls: ['./unactivated.component.sass']
})
export class UnactivatedComponent implements OnInit, OnDestroy{

  private unsubscribe$ = new Subject<void>();
  public isLoading = false;
  constructor(
    private authService: AuthenticationService,
    private userService: UserService,
    private router: Router,
    private notify: NotificationService
  ) { }

  public ngOnInit(): void {
    if (this.authService.getRole() !== RolesEnum.Deactivated) {
      this.router.navigate(['/']);
    }
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public onBtnClick(): void {
    this.isLoading = true;
    this.userService.getUserFromToken().pipe(takeUntil(this.unsubscribe$))
    .subscribe(
      (resp) => {
        if (resp.body.role !== RolesEnum.Deactivated) {
          this.authService.setUser(resp.body);
          this.router.navigate(['/']);
        }
        this.isLoading = false;
      },
      (error) => (this.notify.showError(error))
    );
  }

}
