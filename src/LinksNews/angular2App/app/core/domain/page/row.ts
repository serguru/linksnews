import { Column } from "./column";
import * as _ from 'lodash';
import { ColumnTypes } from "../../common/enums";

export class Row {
    id: number;
    columnId: number;
    rowIndex: number;
    title: string = "New Row";
    showTitle: boolean = false;
    showAd: boolean = false;
    adContent: string;
    columns: Array<Column> = new Array<Column>();
    selected: boolean = false;

    clone(): Row {
        let result: Row = <Row>_.assign(new Row(), this);

        result.columns = new Array<Column>();

        if (this.columns) {
            this.columns.forEach(x => result.columns.push(x.clone()));
        }
        return result;
    }

    static from (row: Row): Row {

        let result: Row = new Row();

        _.assign(result, row);

        result.columns = new Array<Column>();

        if (!row.columns) {
            return result;
        }

        row.columns.forEach(x => result.columns.push(Column.from(x)));
        return result;
    }

    get selectedColumn(): Column {
        if (!this.columns || this.columns.length == 0) {
            return undefined;
        }
        for (let i: number = 0; i < this.columns.length; i++){
            if (this.columns[i].selected){
                return this.columns[i];
            }
        }
        return undefined;
    }

    findColumnById(id: number): Column {
        if (!this.columns) {
            return undefined;
        }
        for (let i: number = 0; i < this.columns.length; i++) {
            let column: Column = this.columns[i];
            if (column.id == id) {
                return column;
            }
        }
        return undefined;
    }


    get leftmostColumnSelected(): boolean {
        let column: Column = this.selectedColumn;
        if (!column){
            return false;
        }
        let result: boolean = this.columns.indexOf(column) == 0;
        return result;
    }

    get rightmostColumnSelected(): boolean {
        let column: Column = this.selectedColumn;
        if (!column){
            return false;
        }
        let result: boolean = this.columns.indexOf(column) == this.columns.length - 1;
        return result;
    }

    shiftSelectedColumnRight(): boolean {
       let column: Column = this.selectedColumn;

        if (!column){
            return false;
        }

        if (this.rightmostColumnSelected){
            return false;
        }

        let index: number = this.columns.indexOf(column);
        this.columns.splice(index,1);
        this.columns.splice(index + 1, 0, column);
    }

    shiftSelectedColumnLeft(): boolean {
    
       let column: Column = this.selectedColumn;

        if (!column){
            return false;
        }

        if (this.leftmostColumnSelected){
            return false;
        }

        let index: number = this.columns.indexOf(column);
        this.columns.splice(index,1);
        this.columns.splice(index - 1, 0, column);
    }

    get totalColumnsWidth(): number {
        if (!this.columns) {
            return 0;
        }

        let result: number = 0;

        this.columns.forEach((x) => {
            result += x.columnWidth;
        });

        return result;
    }

    // TODO: handle column height
    makeColumnNarrower(column: Column){
        if (!column || column.columnWidth <= 1) {
            return;
        }

        column.columnWidth--;
    }

    makeColumnWider(column: Column){
        if (!column || this.totalColumnsWidth >= 12) {
            return;
        }

        column.columnWidth++;
    }

    deleteColumn(column: Column): void {
        if (!this.columns || !column) {
            return;
        }

        let index: number = this.columns.indexOf(column);

        if (index < 0) {
            return;
        }

        let column2select: Column;
        let length = this.columns.length;

        if (length > 1) {
            column2select = index > 0 ? this.columns[index - 1] : this.columns[1];
        }

        this.columns.splice(index, 1);

        this.setColumnSelected(column2select);
    }

    addNewColumn(): Column {
        if (!this.columns) {
            return undefined;
        }

        let occupiedSpace: number = this.totalColumnsWidth;

        if (occupiedSpace >= 12) {
            return undefined;
        }
        
        let freeSpace: number = 12 - occupiedSpace;
        let width: number = freeSpace > 3 ? 3 : freeSpace;
        let column: Column = new Column();
        column.rowId = this.id;
        column.columnWidth = width;
        
        this.columns.push(column);

        this.setColumnSelected(column);

        return column;
    }

    setColumnSelected(column: Column){

        if (!this.columns || !column) {
            return;
        }

        let index: number = this.columns.indexOf(column);

        if (index < 0) {
            return;
        }

        this.columns.forEach((x) => {
            x.selected = x == column;
        });
    }


}
