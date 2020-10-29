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

const routes: Routes = [
  { path: '', component: MainComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login'
  } as GuardData },
  { path: 'profile', component: UserProfileComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login'
  } as GuardData },
  { path: 'history', component: AccessHistoryComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login'
  } as GuardData },
  { path: 'code', component: SecurityCodeListComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login'
  } as GuardData },
  { path: 'history', component: AccessHistoryComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: true,
    redirect: 'login'
  } as GuardData },
  { path: 'login', component: LoginFormComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: false,
    redirect: '/'
  } as GuardData },
  { path: 'registration', component: RegistrationFormComponent, pathMatch: 'full', canActivate: [AuthGuard],
  data: {
    isLogged: false,
    redirect: '/'
  } as GuardData },
  { path: 'reset/:token', component: ResetFormComponent, canActivate: [AuthGuard],
  data: {
    isLogged: false,
    redirect: '/'
  } as GuardData },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
