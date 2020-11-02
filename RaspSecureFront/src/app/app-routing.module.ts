import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { MainComponent } from './components/main/main.component';
import { GuardData } from './models/auth/guard-data';
import { LoginFormComponent } from './components/auth/login-form/login-form.component';
import { RegistrationFormComponent } from './components/auth/registration-form/registration-form.component';
import { ResetFormComponent } from './components/auth/reset-form/reset-form.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { AccessHistoryComponent } from './components/access-history/access-history.component';
import { SecurityCodeListComponent } from './components/security-code-list/security-code-list.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { UnactivatedComponent } from './components/unactivated/unactivated.component';

const routes: Routes = [
  { path: '', component: MainComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login',
    unactivated: true
  } as GuardData },
  { path: 'profile', component: UserProfileComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login',
    unactivated: false
  } as GuardData },
  { path: 'history', component: AccessHistoryComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login',
    unactivated: true
  } as GuardData },
  { path: 'codes', component: SecurityCodeListComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login',
    unactivated: true
  } as GuardData },
  { path: 'users', component: UserListComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login',
    unactivated: true
  } as GuardData },
  { path: 'login', component: LoginFormComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: false,
    redirect: '/',
    unactivated: false
  } as GuardData },
  { path: 'registration', component: RegistrationFormComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: false,
    redirect: '/',
    unactivated: false
  } as GuardData },
  { path: 'reset/:token', component: ResetFormComponent, canActivate: [AuthGuard],
  data: {
    isLogged: false,
    redirect: '/',
    unactivated: false
  } as GuardData },
  { path: 'unactivated', component: UnactivatedComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login',
    unactivated: false
  } as GuardData },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
