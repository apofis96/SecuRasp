import { RolesEnum } from './common/role-enum';

export interface User {
    id: number;
    email: string;
    userName: string;
    role: RolesEnum;
}
