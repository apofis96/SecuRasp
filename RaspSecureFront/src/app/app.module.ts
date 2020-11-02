import { BrowserModule } from '@angular/platform-browser';
import { LOCALE_ID, NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ErrorInterceptor } from './helpers/error.interceptor';
import { JwtInterceptor } from './helpers/jwt.interceptor';
import { LoginFormComponent } from './components/auth/login-form/login-form.component';
import { RegistrationFormComponent } from './components/auth/registration-form/registration-form.component';
import { ResetFormComponent } from './components/auth/reset-form/reset-form.component';
import { SecurityCodeListComponent } from './components/security-code-list/security-code-list.component';
import { AccessHistoryComponent } from './components/access-history/access-history.component';
import { MainComponent } from './components/main/main.component';
import { MaterialModule } from './components/common/material-components.module';
import { HeaderComponent } from './components/header/header.component';
import { ResetDialogComponent } from './components/auth/reset-dialog/reset-dialog.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { RolePipe } from './helpers/role.pipe';
import { registerLocaleData } from '@angular/common';
import localeRu from '@angular/common/locales/ru';
import { EditDialogComponent } from './components/common/edit-dialog/edit-dialog.component';
import { UnactivatedComponent } from './components/unactivated/unactivated.component';

registerLocaleData(localeRu, 'ru');

@NgModule({
  declarations: [
    AppComponent,
    LoginFormComponent,
    RegistrationFormComponent,
    ResetFormComponent,
    SecurityCodeListComponent,
    AccessHistoryComponent,
    MainComponent,
    HeaderComponent,
    ResetDialogComponent,
    UserProfileComponent,
    UserListComponent,
    RolePipe,
    EditDialogComponent,
    UnactivatedComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'ru' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
