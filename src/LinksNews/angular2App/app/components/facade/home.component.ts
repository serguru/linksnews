import { Component, OnInit, OnDestroy } from '@angular/core';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription } from 'rxjs/Subscription';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    
    templateUrl: './home.component.html'
})

export class HomeComponent implements OnInit, OnDestroy {

    subscription: Subscription;
    contentHeight: number;

    constructor (
        public appCommunicateService: AppCommunicateService,
        public router: Router
        ) {
        this.subscription = this.appCommunicateService.contentHeightChanged$.subscribe (
            (height) => {
                this.contentHeight = height;
            }
        )
    }
    go() {
        //this.router.navigate(['/help'], {fragment: 'getStarted'});
        this.router.navigate(['/help']);
    }
    toString(): string {
        return 'HomeComponent';
    }

    get title(): string {
        return "enjoy your own links and fresh news bookmarked";
    }

    ngOnInit() {
        this.appCommunicateService.processCame(this);
    }

    ngOnDestroy() {
        this.appCommunicateService.processGone(this);
        this.subscription.unsubscribe();
    }
}
