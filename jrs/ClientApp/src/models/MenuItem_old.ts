/**
 * Interface for menu items
 * 
 * NOTE: For the moment the menu tree can only go uo to 2 levels. 
 */
export interface MenuItem{
    id: number,
    parentMenuId: number,
    path: string|null,
    label: string|null,
    icon?: string|null,
    url: string,
    labelKey: string,
    subMenuList?: Array<this>,
    ordinalPosition?: number,
    moduleId?: number,
    moduleName?: string,
    moduleCode?: string,
    isHidden?:boolean
}

/**
 * Interface for modules.
 * Module group multiple menus.
 * @param moduleId - Id of the module
 * @param moduleName - Name of the module
 * @param moduleCode - Code of the module
 * @param moduleColor - Color of the module
 */
export interface ImsModule{
    moduleId: number | undefined,
    moduleName: string | undefined,
    moduleCode: string | undefined,
    moduleColor?: string
}

/**
 * List of module colors.
 */
export const moduleColors:Array<any> = [
    { moduleName: "System", moduleColor: "#214885" },
    { moduleName: "Human Resources", moduleColor: "#214885" },
    { moduleName: "Programmes M&E", moduleColor: "#214885" },
    { moduleName: "Implementation", moduleColor: "#214885" },
    { moduleName: "Grants", moduleColor: "#214885" },
    { moduleName: "Detention Centres", moduleColor: "#214885" },
    { moduleName: "Strategic Framework Implementation Plan", moduleColor: "#214885" },
];
