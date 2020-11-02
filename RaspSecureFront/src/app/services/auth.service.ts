import { Injectable } from '@angular/core';
import { HttpInternalService } from './http-internal.service';
import { AccessTokenDto } from '../models/token/access-token-dto';
import { map } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';
import { UserRegisterDto } from '../models/auth/user-register-dto';
import { AuthUser } from '../models/auth/auth-user';
import { UserLoginDto } from '../models/auth/user-login-dto';
import { Observable, of } from 'rxjs';
import { User } from '../models/user';
import { UserService } from './user.service';
import { EventService } from './event.service';
import { RolesEnum } from '../models/common/role-enum';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    public routePrefix = '/api';
    private user: User;

    constructor(private httpService: HttpInternalService, private userService: UserService, private eventService: EventService) { }

    public getUser() {
        return this.user
            ? of(this.user)
            : this.userService.getUserFromToken().pipe(
                map((resp) => {
                    this.user = resp.body;
                    this.eventService.userChanged(this.user);
                    this._setRole(resp.body);
                    return this.user;
                })
            );
    }

    public setUser(user: User) {
        this.user = user;
        this._setRole(user);
        this.eventService.userChanged(user);
    }

    public register(user: UserRegisterDto) {
        return this._handleAuthResponse(this.httpService.postFullRequest<AuthUser>(`${this.routePrefix}/register`, user));
    }

    public isAdminExist() {
        return this.httpService.getFullRequest<boolean>(`${this.routePrefix}/register`);
    }

    public login(user: UserLoginDto) {
        return this._handleAuthResponse(this.httpService.postFullRequest<AuthUser>(`${this.routePrefix}/auth/login`, user));
    }

    public reset(email: string, host: string) {
        return this.httpService.postFullRequest(`${this.routePrefix}/auth/reset/${email}?host=${host}`, null);
    }

    public updatePassword(password: string, token: string) {
        return this.httpService.putFullRequest(`${this.routePrefix}/auth/update/${password}?token=${token}`, null);
    }

    public editPassword(password: string) {
        return this.httpService.putFullRequest(`${this.routePrefix}/auth/edit/${password}`, null);
    }

    public logout() {
        this.revokeRefreshToken();
        this.removeTokensFromStorage();
        this.user = undefined;
        this.eventService.userChanged(undefined);
    }

    public areTokensExist(): boolean {
        return (localStorage.getItem('accessToken') !== null) && (localStorage.getItem('refreshToken') !== null);
    }

    public revokeRefreshToken() {
        return this.httpService.postFullRequest<AccessTokenDto>(`${this.routePrefix}/token/revoke`, {
            refreshToken: localStorage.getItem('refreshToken')
        });
    }

    public removeTokensFromStorage() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('roleToken');
    }

    public refreshTokens() {
        console.log('refresh');
        return this.httpService
            .postRequest<AccessTokenDto>(`${this.routePrefix}/token/refresh`, {
                accessToken: JSON.parse(localStorage.getItem('accessToken')),
                refreshToken: JSON.parse(localStorage.getItem('refreshToken'))
            })
            .pipe(
                map((resp) => {
                    this._setTokens(resp);
                    return resp;
                })
            );
    }

    private _handleAuthResponse(observable: Observable<HttpResponse<AuthUser>>) {
        return observable.pipe(
            map((resp) => {
                this._setTokens(resp.body.token);
                this.user = resp.body.user;
                this._setRole(resp.body.user);
                this.eventService.userChanged(resp.body.user);
                return resp.body.user;
            })
        );
    }

    private _setTokens(tokens: AccessTokenDto) {
        console.log('Try set');
        if (tokens && tokens.accessToken && tokens.refreshToken) {
            console.log(JSON.stringify(tokens.accessToken.token));
            localStorage.setItem('accessToken', JSON.stringify(tokens.accessToken.token));
            localStorage.setItem('refreshToken', JSON.stringify(tokens.refreshToken));
            this.getUser();
        }
    }

    private _setRole(user: User) {
        const tmpToken = localStorage.getItem('roleToken');
        localStorage.setItem('roleToken', JSON.stringify(user.role));
        if (tmpToken && tmpToken !==  user.role.toString()){
            console.log('Attempt');
            this.refreshTokens().subscribe();
        }
        console.log('Done');
        this.eventService.roleChanged();
    }

    public getRole(): RolesEnum {
        return Number(localStorage.getItem('roleToken'));
    }
}
