import { Component, Input, OnInit, OnDestroy, AfterViewChecked } from '@angular/core';
import { DataService } from '../../core/services/data.service';
import { ContentCategory } from '../../core/domain/content-category';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';


@Component({
    selector: 'sidebar-placeholder',
    
    templateUrl: './sidebar.component.html'
})

export class SidebarComponent implements OnInit, OnDestroy, AfterViewChecked {

    @Input() component: any;

    constructor(
        public dataService: DataService,
        public appCommunicateService: AppCommunicateService
        ) { 
    }

    close() {
        
    }


    toString(): string {
        return 'SidebarComponent';
    }

    ngOnInit() {

        
    }

    ngOnDestroy() {
        
    }

    ngAfterViewChecked() {
    }
}

