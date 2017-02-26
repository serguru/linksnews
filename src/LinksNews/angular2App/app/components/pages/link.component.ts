import { Component, Input, ViewChild, AfterViewChecked } from '@angular/core';
import { ColumnComponent } from './column.component';
import { LinkEditorComponent } from './editor/link-editor.component';
import { Link } from '../../core/domain/page/link';
import { DataService } from "../../core/services/data.service";
import { PageComponent } from './page.component';
import { MessengerService } from "../../core/services/messenger.service";

@Component({
    selector: 'link-placeholder',
    
    templateUrl: './link.component.html'
})

export class LinkComponent {

    constructor(
        public dataService: DataService,
        public messengerService: MessengerService 
        ) { 
    }

    @Input() columnComponent: ColumnComponent;

    @Input() link: Link;
    @ViewChild("adPlaceholder") adPlaceholder;

    get linkEditor(): LinkEditorComponent {
        if (!this.columnComponent) {
            return undefined;
        }

        let pageComponent: PageComponent = this.columnComponent.getPageComponent();

        if (!pageComponent) {
            return undefined;
        }

        return pageComponent.linkEditor;
    }

    openLinkEditor() {

        if (this.columnComponent.column.columnTypeId !== 1) {
            return;
        }

        this.linkEditor.openLinkEditor(this);
    }

    get editMode(): boolean {
        return this.columnComponent.editMode;
    }

    refresh() {
        if (!this.link || !this.link.id) {
            return;
        }

        this.dataService.getLinkById(this.link.id,
                    (data) => {
                        let oldLink: Link = this.link;
                        this.link = Link.from(data);

                        if (!this.columnComponent) {
                            return;
                        }
                        let index: number = this.columnComponent.column.links.indexOf(oldLink);
                        this.columnComponent.column.links.splice(index,1);
                        this.columnComponent.column.links.splice(index,0,this.link);
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
        );
    }
}
