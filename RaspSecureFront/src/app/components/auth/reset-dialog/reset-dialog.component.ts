import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-reset-dialog',
  templateUrl: './reset-dialog.component.html',
  styleUrls: ['./reset-dialog.component.sass']
})
export class ResetDialogComponent {

  public email: string;

  constructor(
    public dialogRef: MatDialogRef<ResetDialogComponent>
  ) {}

  onClose(): void {
    this.dialogRef.close();
  }

}
