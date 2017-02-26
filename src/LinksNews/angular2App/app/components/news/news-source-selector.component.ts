import { Component, Input, ViewChild, OnChanges, SimpleChanges, OnInit } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../core/modal/components/modal';
import { MessengerService } from "../../core/services/messenger.service";
import { Column } from "../../core/domain/page/column";
import { NewsSource } from "../../core/domain/news-source";
import { ColumnEditorComponent } from "../pages/editor/column-editor.component";
import { DataService } from '../../core/services/data.service';
import { NewsSourcesComponent } from "./news-sources.component";


@Component({
    selector: 'news-source-selector-placeholder',
    
    templateUrl: './news-source-selector.component.html',
})

export class NewsSourceSelectorComponent implements OnInit {

    zIndex: number;
    maxHeight: number;

    constructor(
        public messengerService: MessengerService,
        public dataService: DataService         
        ) { 
    }

    ngOnInit(){
        this.openPopup();
    }

    @Input("columnEditor") columnEditor: ColumnEditorComponent;

    @ViewChild('newsSourcesComponent') newsSourcesComponent: NewsSourcesComponent;

    @ViewChild('modalNewsSources')
        popup: any;

    @ViewChild('header') header;
    @ViewChild('footer') footer;

    get headerHeight(): number {
        return this.header ? this.header.nativeElement.offsetHeight : 30;
    }

    get footerHeight(): number {
        return this.footer ? this.footer.nativeElement.offsetHeight : 38;
    }

    setMaxHeight() {
        let maxHeight: number =  window.innerHeight - this.headerHeight - this.footerHeight - 150;

        if (this.maxHeight === maxHeight) {
            return;
        }
        this.maxHeight = maxHeight;
    }

    onWindowResize() {
        this.setMaxHeight();
    }

    onPopupOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
        this.newsSourcesComponent.setSelectedSourceById(this.columnEditor.column2edit.newsProviderSourceId);

        setTimeout(() => {
            this.setMaxHeight();
        },0);
    }

    openPopup(/*columnEditor: ColumnEditorComponent*/) {
        //this.columnEditor = columnEditor;
        this.popup.open('lg');
    }

    closePopup() {
        this.popup.dismiss();
    }
    
    selectNewsSource(){
        let source: NewsSource = this.newsSourcesComponent.selectedSource;
        if (!source) {
            this.messengerService.showWarning("Select source", "Please selecte a news source");
            return;
        }
        this.columnEditor.column2edit.newsProviderId = source.newsProviderId;
        this.columnEditor.column2edit.newsProviderSourceId = source.newsSourceId;
        this.popup.dismiss();
    }
}