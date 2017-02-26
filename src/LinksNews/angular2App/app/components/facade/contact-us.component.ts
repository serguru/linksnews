import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription } from 'rxjs/Subscription';
import { Message } from "../../core/domain/message";
import { validationPattern } from "../../core/common/constant";
import { DataService } from "../../core/services/data.service";
import { MessengerService } from "../../core/services/messenger.service";
import { AccountService } from "../../core/services/account.service";

@Component({
    
    templateUrl: './contact-us.component.html'
})

export class ContactUsComponent implements OnInit, OnDestroy {

    pattern: any = validationPattern;

    //id: number;
    //messageGroupId: number;
    //parentMessageId: number;
    //sentDate: Date;
    //sentFromIP: string;
    //pageId: number;
    //authorAccountId: number;
    //authorName: string;
    //authorEmail: string;
    //subject: string;
    //messageText: string;

    public message: Message;

    subscription: Subscription;
    contentHeight: number;

    constructor (
        public appCommunicateService: AppCommunicateService,
        public dataService: DataService,
        public messengerService: MessengerService,
        public accountService: AccountService
        ) {
        this.subscription = this.appCommunicateService.contentHeightChanged$.subscribe (
            (height) => {
                this.contentHeight = height;
            }
        )
    }

    toString(): string {
        return 'ContactUsComponent';
    }

    get title(): string {
        return "contact us";
    }

    ngOnInit() {
        var message: Message = new Message();
        var account = this.accountService.account;

        if (account) {
            message.authorAccountId = account.id;
            message.authorEmail = account.email;

            if (account.firstName) {
                message.authorName = account.firstName  +
                    (account.lastName ? " " + account.lastName : "");

            } else {
                message.authorName = account.login;
            }
        }

        this.message = message;
        this.appCommunicateService.processCame(this);
    }

    ngOnDestroy() {
        this.appCommunicateService.processGone(this);
        this.subscription.unsubscribe();
    }

    send(): void {
    
        this.dataService.sendContactUs(this.message, 
            () => {
                this.messengerService.showOK("Contact us", "Message has been sent");
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
            );
    
    }
}
