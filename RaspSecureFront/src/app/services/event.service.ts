import { Injectable } from '@angular/core';

import { Subject } from 'rxjs';
import { User } from '../models/user';

@Injectable({ providedIn: 'root' })
export class EventService {
    private onUserChanged = new Subject<User>();
    public userChangedEvent$ = this.onUserChanged.asObservable();

    private onRoleChanged = new Subject<void>();
    public roleChangedEvent$ = this.onRoleChanged.asObservable();

    public userChanged(user: User) {
        this.onUserChanged.next(user);
    }

    public roleChanged() {
        this.onRoleChanged.next();
    }
}
