import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'hideIfFalse'})
export class HideIfFalsePipe implements PipeTransform {
  transform(value: boolean): string {
    return value ? "" : "none";
  }
}
