import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
    
    templateUrl: './privacy-policy.component.html'
})

export class PrivacyPolicyComponent implements OnInit, OnDestroy {

    subscription: Subscription;
    contentHeight: number;

    constructor (
        public appCommunicateService: AppCommunicateService
        ) {
        this.subscription = this.appCommunicateService.contentHeightChanged$.subscribe (
            (height) => {
                this.contentHeight = height;
            }
        )
    }

    get title(): string {
        return "privacy policy";
    }


    toString(): string {
        return 'PrivacyPolicyComponent';
    }

    ngOnInit() {
        this.appCommunicateService.processCame(this);
    }

    ngOnDestroy() {
        this.appCommunicateService.processGone(this);
        this.subscription.unsubscribe();
    }
}
