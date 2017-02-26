import { Directive, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, Validator, ValidatorFn, Validators } from '@angular/forms';

export function urlStartValidator(): ValidatorFn {

    let prefixes = [
        "http://",
        "https://",
        "file://"
    ]

    return (control: AbstractControl): { [key: string]: any } => {
        let value = control.value;

        if (!value || value.length === 0) {
            return null;
        }

        value = value.toLowerCase();

        for (let i: number = 0; i < prefixes.length; i++) {
            if (value.startsWith(prefixes[i])) {
                return null;
            }
        }

        return { 'urlStart': { value } };
    };
}

@Directive({
    selector: '[urlStart]',
    providers: [{ provide: NG_VALIDATORS, useExisting: UrlStartValidatorDirective, multi: true }]
})
export class UrlStartValidatorDirective implements Validator, OnChanges {

    @Input() urlStart: string;
    public valFn = Validators.nullValidator;

    ngOnChanges(changes: SimpleChanges): void {
        const change = changes['urlStart'];
        if (change) {
            this.valFn = urlStartValidator();
        } else {
            this.valFn = Validators.nullValidator;
        }
    }

    validate(control: AbstractControl): { [key: string]: any } {
        return this.valFn(control);
    }
}

