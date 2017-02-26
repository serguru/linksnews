import { DialogButtonConfig } from "./dialog-button-config";

export class MessageConfig {
    constructor(
        public content: string,
        public header?: string,
        public footer?: string,
        public buttons?: Array<DialogButtonConfig>
    ){}
}