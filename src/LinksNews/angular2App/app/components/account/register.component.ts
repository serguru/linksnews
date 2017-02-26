import { Component, OnInit, AfterViewChecked } from '@angular/core';
import { Router } from '@angular/router';
import { RegistrationData } from '../../core/domain/registration-data';
import { OperationResult } from '../../core/domain/operation-result';
import { AccountService } from '../../core/services/account.service';
import { MessengerService } from "../../core/services/messenger.service";
import { validationPattern } from "../../core/common/constant";

@Component({
    //providers: [AccountService],
    
    templateUrl: './register.component.html'
})

export class RegisterComponent implements OnInit, AfterViewChecked {

    pattern: any = validationPattern;

    public registration: RegistrationData = new RegistrationData();

    constructor(public accountService: AccountService,
            public messengerService: MessengerService, 
            public router: Router) { 
    }

    ngOnInit() {
        //this.registration = new RegistrationData('', '', '','','','');
        //this.registration = new RegistrationData();
    }

    valid(): boolean {
        if (this.registration.password != this.registration.passwordConfirm) {
            this.messengerService.showWarning("Register", "Password does not match password confirmation");
            return false;
        }

        return true;
    }

    register(backUrl?: string): void {
        if (!this.valid()) {
            return;
        }

        this.messengerService.showWait("Registering...", () => {
        
            this.accountService.register(this.registration, 
                () => {
                    let url = backUrl || "/myPages";
                    this.router.navigate([url]);
                    this.messengerService.closeWait();
                },
                () => {
                    this.messengerService.closeWait();
                });
        });

    }

    alignVertical: boolean = true;

    windowHeight: number;

    setVerticalAlign() {
        setTimeout(() => {
            if (!this.windowHeight) {
                return;
            }

            this.alignVertical = this.windowHeight > 500;
        },0);
    }

    ngAfterViewChecked(){
        if (window.innerHeight !== this.windowHeight) {
            this.windowHeight = window.innerHeight;
            this.setVerticalAlign();
        }
    }



    onWindowResize(event) {
        this.setVerticalAlign();
    }
}
