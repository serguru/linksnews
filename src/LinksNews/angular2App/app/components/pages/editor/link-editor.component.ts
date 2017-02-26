import { Component, Input, ViewChild, AfterViewChecked } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Link } from "../../../core/domain/page/link";
import { LinkComponent } from "../link.component";
import { ColumnTypes } from "../../../core/common/enums";
import { FileUploadComponent } from "./file-upload.component";
import { AccountService } from '../../../core/services/account.service';

@Component({
    selector: 'link-editor-placeholder',
    
    templateUrl: './link-editor.component.html'
})

export class LinkEditorComponent {

    @ViewChild('modalLinkEditor') linkEditor: any;
    @ViewChild('uploader') uploader: FileUploadComponent;
    @ViewChild('header') header;
    @ViewChild('footer') footer;

    zIndex: number;
    link2edit: Link;
    maxHeight: number;

    linkComponent: LinkComponent;

    constructor(
        public messengerService: MessengerService, 
        public dataService: DataService,
        public accountService: AccountService
        ) { 
    }

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

    onEditorOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
        setTimeout(() => {
            this.setMaxHeight();
        },0);
    }

    openLinkEditor(linkComponent: LinkComponent) {

        this.linkComponent = linkComponent;

        if (this.linkComponent.columnComponent.column.columnTypeId != ColumnTypes.Links){
            return;
        }
        this.link2edit = this.linkComponent.link.clone();
        this.linkEditor.open('lg');
    }

    closeLinkEditor() {
        this.linkEditor.dismiss();
    }

    onEditorDismiss(){
    }


    saveLink() {
        this.dataService.saveLink(this.link2edit, 
                    (data) => {
                        this.linkEditor.dismiss().then(() => {
                                this.messengerService.showOK("Link save", "Changes have been saved" , () => {
                                    this.linkComponent.refresh();
                                });
                            }
                        );
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
                );
    }

    saveLinkAndImage() {

        if (!this.link2edit.imageUrl && !this.uploader.file) {
            this.dataService.deleteImage4Link(this.link2edit.id, 
                    () => {
                        this.saveLink();
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
                );
            return;
        }

        let fileToUpload = this.uploader.file;

        if (fileToUpload) {
            
            this.dataService.uploadImage4Link(this.link2edit.id, fileToUpload,
                    () => {
                        this.saveLink();
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
                );

            return;
        }

        this.saveLink();
    }

}