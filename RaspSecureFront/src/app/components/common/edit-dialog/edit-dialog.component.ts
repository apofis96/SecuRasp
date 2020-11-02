import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EditDialogData } from 'src/app/models/common/edit-dialog';
import { EditEnum } from 'src/app/models/common/edit-dialog-enum';
import { RolesEnum } from 'src/app/models/common/role-enum';

@Component({
  selector: 'app-edit-dialog',
  templateUrl: './edit-dialog.component.html',
  styleUrls: ['./edit-dialog.component.sass']
})
export class EditDialogComponent {

  public title = 'Изменить';

  constructor(
    public dialogRef: MatDialogRef<EditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EditDialogData) {
      switch (this.data.type) {
        case EditEnum.Email:
          this.title += ' Почту';
          break;
        case EditEnum.Name:
          this.title += ' Имя';
          break;
        case EditEnum.Role:
          this.title += ' Роль';
          this.data.value = (this.data.value as RolesEnum).toString();
          break;
      }
    }

  onNoClick(): void {
    this.dialogRef.close();
  }

}
