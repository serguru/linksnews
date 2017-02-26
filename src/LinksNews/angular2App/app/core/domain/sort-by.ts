import { SortOrder } from "../common/enums";

export class SortBy {
    constructor(public name: string, public sortOrder: SortOrder) {
    }

    next(): SortOrder {
        if (this.sortOrder == SortOrder.Desc) {
            return SortOrder.None;
        }

        return this.sortOrder + 1;
    }
}
