import { Pipe, PipeTransform } from '@angular/core';
import { RolesEnum } from '../models/common/role-enum';

@Pipe({
    name: 'role'
})
export class RolePipe implements PipeTransform {
  transform(value: RolesEnum): string {
      console.log(value);
    switch (value) {
        case RolesEnum.Deactivated:
            return 'Неактивированный';
        case RolesEnum.Admin:
            return 'Администратор';
        case RolesEnum.Editor:
            return 'Редактирование';
        case RolesEnum.Observer:
            return 'Просмотр';
        case RolesEnum.Terminal:
            return 'Терминал';
    }
    }
}
