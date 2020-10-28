import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ErrorInterceptor } from './helpers/error.interceptor';
import { JwtInterceptor } from './helpers/jwt.interceptor';
import { LoginFormComponent } from './componets/auth/login-form/login-form.component';
import { RegistrationFormComponent } from './componets/auth/registration-form/registration-form.component';
import { ResetFormComponent } from './componets/auth/reset-form/reset-form.component';
import { SecurityCodeListComponent } from './componets/security-code-list/security-code-list.component';
import { AccessHistoryComponent } from './componets/access-history/access-history.component';
import { MainComponent } from './componets/main/main.component';
import { MaterialModule } from './componets/common/material-components.module';
import { HeaderComponent } from './componets/header/header.component';
import { ResetDialogComponent } from './componets/auth/reset-dialog/reset-dialog.component';
import { UserProfileComponent } from './componets/user-profile/user-profile.component';

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
    UserProfileComponent
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
  exports: [
    MaterialModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
