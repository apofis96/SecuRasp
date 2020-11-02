import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { RolesEnum } from 'src/app/models/common/role-enum';
import { User } from 'src/app/models/user';
import { AuthenticationService } from 'src/app/services/auth.service';
import { EventService } from 'src/app/services/event.service';
import { NotificationService } from 'src/app/services/notification.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.sass']
})
export class HeaderComponent implements OnInit, OnDestroy {

  public authorizedUser: User;
  public navigationAllowed = false;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private authService: AuthenticationService,
    private eventService: EventService,
    private userService: UserService,
    private router: Router,
    private notify: NotificationService) { }

  ngOnInit(): void {
    this.getUser();
    this.eventService.userChangedEvent$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((user) => {
        this.authorizedUser = user ? this.userService.copyUser(user) : undefined;
      });
    this.eventService.roleChangedEvent$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        this.checkNavigation();
      });
    this.checkNavigation();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['login']);
  }

  private getUser() {
    if (!this.authService.areTokensExist()) {
        return;
    }
    this.authService
        .getUser()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((user) => {
          this.authorizedUser = this.userService.copyUser(user);
        },
          (error) => (this.notify.showError(error))
        );
  }

  private checkNavigation(): void {
    if (this.authService.areTokensExist() && this.authService.getRole() !== RolesEnum.Deactivated) {
      this.navigationAllowed = true;
    }
    else {
      this.navigationAllowed = false;
    }
  }

}
