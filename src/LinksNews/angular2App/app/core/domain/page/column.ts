import { ColumnTypes } from "../../common/enums";
import { Row } from "./row";
import { Link } from "./link";
import * as _ from 'lodash';
import { ViewModes } from "../../common/enums";

export class Column {
    id: number;
    columnTypeId: ColumnTypes = ColumnTypes.Links;
    rowId: number;
    pageId: number;
    columnIndex: number;
    columnWidth: number = 1;
    title: string = "New Column";
    description: string;
    imageUrl: string;
    newsProviderId: number;
    newsProviderSourceId: string;
    showTitle: boolean = true;
    showImage: boolean = false;
    showDescription: boolean = false;
    showNewsImages: boolean = true;
    showNewsDescriptions: boolean = true;
    viewModeId: ViewModes = ViewModes.List;
    showAd: boolean = false;
    adContent: string;
    selected: boolean = false;
    rows: Array<Row> = new Array<Row>();
    links: Array<Link> = new Array<Link>();


    clone(): Column {
        let result: Column = <Column>_.assign(new Column(), this);

        result.rows = new Array<Row>();
        if (this.rows) {
            this.rows.forEach(x => result.rows.push(x.clone()));
        }

        result.links = new Array<Link>();
        if (this.columnTypeId != ColumnTypes.News && this.links) {
            this.links.forEach(x => {
                if (x.newsLink) {
                    return;
                }
                result.links.push(x.clone());
            });
        }

        return result;
    }

    cloneNoChildren(): Column {
        let result: Column = <Column>_.assign(new Column(), this);
        result.rows = new Array<Row>();
        result.links = new Array<Link>();
        return result;
    }

    static from (column: Column): Column {
        
        let result: Column = new Column();
        
        _.assign(result, column);

        result.rows = new Array<Row>();

        if (column.rows) {
            column.rows.forEach(x => result.rows.push(Row.from(x)));
        }
    
        result.links = new Array<Link>();

        if (!column.links) {
            return result;
        }

        column.links.forEach(x => {
            result.links.push(Link.from(x));
        });
        return result;
    }

    get selectedRow(): Row {
        if (!this.rows || this.rows.length == 0) {
            return undefined;
        }
        for (let i: number = 0; i < this.rows.length; i++){
            if (this.rows[i].selected){
                return this.rows[i];
            }
        }
        return undefined;
    }

    get selectedLink(): Link {
        if (!this.links || this.links.length == 0) {
            return undefined;
        }
        for (let i: number = 0; i < this.links.length; i++){
            if (this.links[i].selected){
                return this.links[i];
            }
        }
        return undefined;
    }

    addNewRow(): Row {
    
        if (!this.rows || this.rows.length > 100) {
            return undefined;
        }
        
        let row: Row = new Row();
        row.columnId = this.id;
        
        this.rows.push(row);

        this.setRowSelected(row);

        return row;
    }

    addNewLink(): Link {
    
        if (!this.links || this.links.length > 100) {
            return undefined;
        }
        
        let link: Link = new Link();
        link.columnId = this.id;
        
        this.links.push(link);

        this.setLinkSelected(link);

        return link;
    }

    deleteRow(row: Row): void {
        if (!this.rows || !row) {
            return;
        }

        let index: number = this.rows.indexOf(row);

        if (index < 0) {
            return;
        }

        let row2select: Row;
        let length = this.rows.length;

        if (length > 1) {
            row2select = index > 0 ? this.rows[index - 1] : this.rows[1];
        }

        this.rows.splice(index, 1);

        this.setRowSelected(row2select);
    }

    deleteLink(link: Link): void {
        if (!this.links || !link) {
            return;
        }

        let index: number = this.links.indexOf(link);

        if (index < 0) {
            return;
        }

        let link2select: Link;
        let length = this.links.length;

        if (length > 1) {
            link2select = index > 0 ? this.links[index - 1] : this.links[1];
        }

        this.links.splice(index, 1);

        this.setLinkSelected(link2select);
    }

    setRowSelected(row: Row) {

        if (!this.rows || !row) {
            return;
        }

        let index: number = this.rows.indexOf(row);

        if (index < 0) {
            return;
        }

        this.rows.forEach((x) => {
            x.selected = x == row;
        });
    }

    setLinkSelected(link: Link) {

        if (!this.links || !link) {
            return;
        }

        let index: number = this.links.indexOf(link);

        if (index < 0) {
            return;
        }

        this.links.forEach((x) => {
            x.selected = x == link;
        });
    }

    onShowNewsSourcesClick() {
    }

    get topmostRowSelected(): boolean {
        let row: Row = this.selectedRow;
        if (!row){
            return false;
        }
        let result: boolean = this.rows.indexOf(row) == 0;
        return result;
    }

    get lowermostRowSelected(): boolean {
        let row: Row = this.selectedRow;
        if (!row){
            return false;
        }
        let result: boolean = this.rows.indexOf(row) == this.rows.length - 1;
        return result;
    }

    shiftSelectedRowDown(): boolean {
        let row: Row = this.selectedRow;
        if (!row){
            return false;
        }

        if (this.lowermostRowSelected) {
            return false;
        }

        let index: number = this.rows.indexOf(row);
        this.rows.splice(index,1);
        this.rows.splice(index + 1, 0, row);
    }

    shiftSelectedRowUp(): boolean {
    
        let row: Row = this.selectedRow;
        if (!row){
            return false;
        }

        if (this.topmostRowSelected){
            return false;
        }

        let index: number = this.rows.indexOf(row);
        this.rows.splice(index,1);
        this.rows.splice(index - 1, 0, row);
    }

    get topmostLinkSelected(): boolean {
        let link: Link = this.selectedLink;
        if (!link){
            return false;
        }
        let result: boolean = this.links.indexOf(link) == 0;
        return result;
    }

    get lowermostLinkSelected(): boolean {
        let link: Link = this.selectedLink;
        if (!link){
            return false;
        }
        let result: boolean = this.links.indexOf(link) == this.links.length - 1;
        return result;
    }

    shiftSelectedLinkDown(): boolean {
        let link: Link = this.selectedLink;
        if (!link){
            return false;
        }

        if (this.lowermostLinkSelected) {
            return false;
        }

        let index: number = this.links.indexOf(link);
        this.links.splice(index,1);
        this.links.splice(index + 1, 0, link);
    }

    shiftSelectedLinkUp(): boolean {
    
        let link: Link = this.selectedLink;
        if (!link){
            return false;
        }

        if (this.topmostLinkSelected){
            return false;
        }

        let index: number = this.links.indexOf(link);
        this.links.splice(index,1);
        this.links.splice(index - 1, 0, link);
    }

    
    findRowById(id: number): Row {
        if (!this.rows) {
            return undefined;
        }
        for (let i: number = 0; i < this.rows.length; i++) {
            let row: Row = this.rows[i];
            if (row.id == id) {
                return row;
            }
        }
        return undefined;
    }

    findLinkById(id: number): Link {
        if (!this.links) {
            return undefined;
        }
        for (let i: number = 0; i < this.links.length; i++) {
            let link: Link = this.links[i];
            if (link.id == id) {
                return link;
            }
        }
        return undefined;
    }



}
