import { Component, OnInit, AfterViewChecked} from '@angular/core';
import { Router } from '@angular/router';
import { Account } from '../../core/domain/account';
import { OperationResult } from '../../core/domain/operation-result';
import { AccountService } from '../../core/services/account.service';
import { MessengerService } from "../../core/services/messenger.service";
import { validationPattern } from "../../core/common/constant";

@Component({
    
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit, AfterViewChecked {
    

    //^[0-9a-zA-Z$_.+!*'()-]+$

    pattern: any = validationPattern;

    get title(): string {
        return "login";
    }


    login: string = "";
    password: string = "";

    constructor(public accountService: AccountService,
                public router: Router,
                public messengerService: MessengerService
                ) { }

    ngOnInit() {
        
    }

    logUserIn(backUrl?: string): void {

        this.messengerService.showWait("Authorizing...", () => {
            this.accountService.login(this.login, this.password, 
                () => {
                    let url = backUrl || "/myPages";
                    this.router.navigate([url]);
                    this.messengerService.closeWait();
                },
                () => {
                    this.messengerService.closeWait();
                }
            );
        });

    }

    onForgotPasswordClick() {
        this.messengerService.showOK("Forgot password", "Please send us a message. We will send your new password to your email address.", () => {
            this.router.navigate(["/contactUs"]);           
        })

    }

    alignVertical: boolean = true;

    windowHeight: number;

    setVerticalAlign() {
        setTimeout(() => {
            if (!this.windowHeight) {
                return;
            }

            this.alignVertical = this.windowHeight > 400;
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