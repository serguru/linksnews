import { Directive, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, Validator, ValidatorFn, Validators } from '@angular/forms';

export function spaceValidator(): ValidatorFn {

    let forbiddenCharacters = [
        String.fromCharCode(9), // tab
        String.fromCharCode(32), // space
        String.fromCharCode(10), // line feed
        String.fromCharCode(13), // carriage return
    ]

    return (control: AbstractControl): { [key: string]: any } => {
        const value = control.value;
        const no = value && value.length > 0 &&
            (
                forbiddenCharacters.indexOf(value[0]) >= 0
                ||
                forbiddenCharacters.indexOf(value[value.length - 1]) >= 0
            );
        return no ? { 'wrongSpace': { value } } : null;
    };
}

@Directive({
    selector: '[wrongSpace]',
    providers: [{ provide: NG_VALIDATORS, useExisting: SpaceValidatorDirective, multi: true }]
})
export class SpaceValidatorDirective implements Validator, OnChanges {

    @Input() wrongSpace: string;
    public valFn = Validators.nullValidator;

    ngOnChanges(changes: SimpleChanges): void {
        const change = changes['wrongSpace'];
        if (change) {
            this.valFn = spaceValidator();
        } else {
            this.valFn = Validators.nullValidator;
        }
    }

    validate(control: AbstractControl): { [key: string]: any } {
        return this.valFn(control);
    }
}

//import { Directive, Input, OnChanges, SimpleChanges } from '@angular/core';
//import { AbstractControl, NG_VALIDATORS, Validator, ValidatorFn, Validators } from '@angular/forms';

///** A hero's name can't match the given regular expression */
//export function forbiddenNameValidator(nameRe: RegExp): ValidatorFn {
//  return (control: AbstractControl): {[key: string]: any} => {
//    const name = control.value;
//    const no = nameRe.test(name);
//    return no ? {'forbiddenName': {name}} : null;
//  };
//}

//@Directive({
//  selector: '[forbiddenName]',
//  providers: [{provide: NG_VALIDATORS, useExisting: ForbiddenValidatorDirective, multi: true}]
//})
//export class ForbiddenValidatorDirective implements Validator, OnChanges {
//  @Input() forbiddenName: string;
//  public valFn = Validators.nullValidator;

//  ngOnChanges(changes: SimpleChanges): void {
//    const change = changes['forbiddenName'];
//    if (change) {
//      const val: string | RegExp = change.currentValue;
//      const re = val instanceof RegExp ? val : new RegExp(val, 'i');
//      this.valFn = forbiddenNameValidator(re);
//    } else {
//      this.valFn = Validators.nullValidator;
//    }
//  }

//  validate(control: AbstractControl): {[key: string]: any} {
//    return this.valFn(control);
//  }
//}

