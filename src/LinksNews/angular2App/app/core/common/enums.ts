export enum DialogButtonType {
    OK,
    Yes,
    No,
    Cancel,
    Close
}

export enum ColumnTypes {
    Links = 1, 
    News = 2,
    Rows = 3 
}


export enum RowAction { 
    Unchanged, 
    Inserted, 
    Updated, 
    Deleted
};


export enum PageElement { 
    Page, 
    Column, 
    Row, 
    Link
};

export enum ViewModes {
    List = 1, 
    Tile = 2
}

export enum SortOrder {
    None,
    Asc,
    Desc
}
