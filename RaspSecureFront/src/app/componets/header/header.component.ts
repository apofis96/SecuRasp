import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { User } from 'src/app/models/user';
import { AuthenticationService } from 'src/app/services/auth.service';
import { EventService } from 'src/app/services/event.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.sass']
})
export class HeaderComponent implements OnInit, OnDestroy {

  public authorizedUser: User;
  private unsubscribe$ = new Subject<void>();
  private currentUrl: string;

  constructor(
    private authService: AuthenticationService,
    private eventService: EventService,
    private userService: UserService,
    private router: Router) { }

  ngOnInit(): void {
    this.getUser();
    this.eventService.userChangedEvent$
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe((user) => (this.authorizedUser = user ? this.userService.copyUser(user) : undefined));
    this.router.events.subscribe((val) => {if (val instanceof NavigationEnd) {
  this.currentUrl = val.urlAfterRedirects;}});
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
        .subscribe((user) => (this.authorizedUser = this.userService.copyUser(user)));
}

}
