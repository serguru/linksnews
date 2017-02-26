import { Component, Input, ViewChild, OnChanges, SimpleChanges} from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Link } from "../../../core/domain/page/link";
import { LinkComponent } from "../link.component";
import { ColumnTypes } from "../../../core/common/enums";

@Component({
    selector: 'file-upload-placeholder',
    
    templateUrl: './file-upload.component.html'
})

export class FileUploadComponent implements OnChanges {

    @Input() parent: any;

    imageUrl: string;

    @Input() sizeLimit: number; // file size limit in Kb

    deleted: boolean = false;

    file: any;

    @ViewChild('fileInput') fileInput: any;

    constructor(public messengerService: MessengerService, public dataService: DataService) { 
    }

    get cancelOrDeleteDisabled(): boolean {
        return !this.file && !this.parent.imageUrl;
    }

    public resetFileInput() {
        this.fileInput.nativeElement.value = "";
    }

    cancelOrDelete(): void {

        this.resetFileInput();
        this.file = undefined;

        // cancel image selection
        if (this.file) {
            this.imageUrl = this.parent.imageUrl;
            return;
        }          
        
        // delete parent's image url
        this.parent.imageUrl = undefined;
        this.imageUrl = undefined;
    }

    get cancelOrDeleteTitle(): string {
        if (this.file) {
            return 'Cancel';
        }          

        return 'Delete';
    }

    ngOnChanges(changes: SimpleChanges){
        this.file = undefined;
        this.imageUrl = this.parent.imageUrl;
    }

    onFileInputChange(e) {

        this.file = undefined;

        if (typeof FileReader === "undefined" || e.target.files === undefined) {
            this.messengerService.showError("File read error", "Cannot read a file");
            return;
        }

        let files: any = e.target.files;

        e.stopPropagation();

        if (files.length === 0) {
            return;
        }

        let file = files[0];

        if (typeof file.type === "undefined" || !file.type.match(/^image\/(x-png|pjpeg|jpeg|bmp|png|gif)$/)) {
            this.messengerService.showWarning("Wrong image type", "File selected is not an image");
            return;
        }

        if (file.size > (this.sizeLimit || 5) * 1024 * 1024) {
            this.messengerService.showError("File upload error", "File size exceeds a limit " + (this.sizeLimit || 5) + " Mb");
            return;
        }

        let reader = new FileReader();

        reader.onload = (x) => {
            this.imageUrl = (<any>x.target).result;
            this.file = file;
        }

        reader.readAsDataURL(file);
    }
}