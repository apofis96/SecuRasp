import { Injectable } from '@angular/core';
import { HttpInternalService } from '../services/http-internal.service';
import { User } from '../models/user';
import { UsersList } from '../models/userList';

@Injectable({ providedIn: 'root' })
export class UserService {
    public routePrefix = '/api/users';

    constructor(private httpService: HttpInternalService) {}

    public getUserFromToken() {
        return this.httpService.getFullRequest<User>(`${this.routePrefix}/fromToken`);
    }

    public getUserById(id: number) {
        return this.httpService.getFullRequest<User>(`${this.routePrefix}`, { id });
    }

    public updateUser(user: User) {
        return this.httpService.putFullRequest<User>(`${this.routePrefix}`, user);
    }

    public copyUser({ email, userName, id, role, createdAt}: User) {
        return {
            email,
            userName,
            id,
            role,
            createdAt
        };
    }

    public getUsers(sort: string, page: number, size: number, unactive: boolean) {
        return this.httpService.getFullRequest<UsersList>(
            `${this.routePrefix}?sort=${sort}&page=${page}&size=${size}&unactive=${unactive}`
        );
    }

    public adminUpdateUser(user: User) {
        return this.httpService.putFullRequest<User>(`${this.routePrefix}/adminEdit`, user);
    }
}
