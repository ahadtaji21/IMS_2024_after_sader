export enum snackbarCategories{
    info = 'info',
    success = 'success',
    warning = 'warning',
    error = 'error'
}

export interface ISnackbarType{
    category: snackbarCategories,
    color:string
}

export const snackbarTypes:Array<ISnackbarType> = [
    {
        category: snackbarCategories.info,
        color: 'info darken-4'
    },
    {
        category: snackbarCategories.success,
        color: 'success darken-4'
    },
    {
        category: snackbarCategories.warning,
        color: 'warning darken-3'
    },
    {
        category: snackbarCategories.error,
        color: 'error darken-3'
    }
];

export interface ISnackbar{
    type: ISnackbarType,
    text: String
}

