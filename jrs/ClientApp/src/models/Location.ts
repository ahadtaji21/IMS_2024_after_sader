
export interface AdminArea{
    IMS_ADMIN_AREA_ID: number,
    IMS_ADMIN_AREA_NAME: string,
    IMS_ADMIN_AREA_NODE:string

}

export interface Location{
    IMS_LOCATION_ID: number,
    IMS_LOCATION_ADMIN_AREA_ID_1: number,
    IMS_LOCATION_ADMIN_AREA_ID_2: number,
    IMS_LOCATION_ADMIN_AREA_ID_3: number,
    IMS_LOCATION_ADDRESS: string,
    IMS_LOCATION_POSTAL_CODE?: string
}

