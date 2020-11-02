import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  public constructor(private snackBar: MatSnackBar) {}

  public showError(error: any): void {
      this.snackBar.open(error, '', { duration: 5000, panelClass: 'error-snack-bar', horizontalPosition: 'right',
      verticalPosition: 'bottom' });
  }

  public showNotification(message: any): void {
      this.snackBar.open(message, '', { duration: 5000, horizontalPosition: 'right',
      verticalPosition: 'bottom' });
  }
}
