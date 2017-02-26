import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataService } from '../../core/services/data.service';
import { ActivatedRoute, UrlSegment } from '@angular/router';
import { Page } from "../../core/domain/page/page";
import { MessengerService } from "../../core/services/messenger.service";
import { Router } from '@angular/router';
import { AccountService } from "../../core/services/account.service";
import { Account } from "../../core/domain/account";
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';
import { ViewModes }   from '../../core/common/enums';
import { UtilityService } from '../../core/services/utility.service';

//import * as _ from 'lodash';  
@Component({
    selector: 'pages-list-placeholder',
    
    templateUrl: './pages-list.component.html'
})

export class PagesListComponent implements OnInit, OnDestroy {

    constructor (
                public dataService: DataService,
                public route: ActivatedRoute,
                public messengerService: MessengerService,
                public accountService: AccountService, 
                public router: Router,
                public utilityService: UtilityService,
                public appCommunicateService: AppCommunicateService
        ) {
    }

    @Input() pages: Array<Page>;
    @Input() viewMode: ViewModes;

    ngOnInit(){
    }

    ngOnDestroy(){
    }
}
