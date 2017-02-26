import { Component, Input, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Column } from "../../../core/domain/page/column";
import { Row } from "../../../core/domain/page/row";
import { ColumnTypes } from "../../../core/common/enums";
import { NewsSourceSelectorComponent } from "../../news/news-source-selector.component";


@Component({
    selector: 'column-presenter-placeholder',
    
    templateUrl: './column-presenter.component.html'
})



export class ColumnPresenterComponent {

    @Input()
        column: Column;

    @Input()
        row: Row;

    constructor(public messengerService: MessengerService, public dataService: DataService) { 
    }
}