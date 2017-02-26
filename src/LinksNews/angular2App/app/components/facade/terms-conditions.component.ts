import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
    
    templateUrl: './terms-conditions.component.html'
})

export class TermsConditionsComponent implements OnInit, OnDestroy {

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

    toString(): string {
        return 'TermsConditionsComponent';
    }

    get title(): string {
        return "terms and conditions";
    }

    ngOnInit() {
        this.appCommunicateService.processCame(this);
    }

    ngOnDestroy() {
        this.appCommunicateService.processGone(this);
        this.subscription.unsubscribe();
    }
}
