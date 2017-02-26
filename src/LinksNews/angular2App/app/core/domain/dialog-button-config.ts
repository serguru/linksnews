import { DialogButtonType } from "../common/enums";

export class DialogButtonConfig {
    constructor(
        public buttonType: DialogButtonType,
        public handler?: Function
    ){}
}