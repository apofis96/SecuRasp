import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../services/auth.service';
import { GuardData } from '../models/auth/guard-data';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivateChild, CanActivate {
    constructor(private router: Router, private authService: AuthenticationService) {}

    public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.checkForActivation(state, route.data as GuardData);
    }

    public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        return this.checkForActivation(state, route.data as GuardData);
    }

    private checkForActivation(state: RouterStateSnapshot, guardData: GuardData) {

        if (this.authService.areTokensExist() === guardData.isLogged) {
            return true;
        }

        this.router.navigate(['/' + guardData.redirect]);

        return false;
    }
}
