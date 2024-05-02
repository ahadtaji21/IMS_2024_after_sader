import {
  GenericExcelColumnLabel,
  GenericSqlSelectPayload,
  GenericSqlPayload,
} from "../axiosapi/api";
import { mapActions, mapGetters } from "vuex";
import { JrsHeader } from "@/models/JrsTable";
import { translate } from "../i18n";
export default {
  filters: {
    formatDate: function(string: any) {
      if (string != undefined) return string.split("T")[0];
      else return "";
    },
  },
  methods: {
    ...mapActions("apiHandler", {
      getJRSTableStructure: "getJRSTableStructure",
      execSPCall: "execSPCall",
    }),
    ...mapActions("apiHandler", {
      generateExcelForTable: "generateExcelForTable",
      generateExcelForOverviewIndicators: "generateExcelForOverviewIndicators",
    }),
    makeUUID() {
      var hex = [];

      for (var i = 0; i < 256; i++) {
        hex[i] = (i < 16 ? "0" : "") + i.toString(16);
      }
      var r = crypto.getRandomValues(new Uint8Array(16));

      r[6] = (r[6] & 0x0f) | 0x40;
      r[8] = (r[8] & 0x3f) | 0x80;

      return (
        hex[r[0]] +
        hex[r[1]] +
        hex[r[2]] +
        hex[r[3]] +
        "-" +
        hex[r[4]] +
        hex[r[5]] +
        "-" +
        hex[r[6]] +
        hex[r[7]] +
        "-" +
        hex[r[8]] +
        hex[r[9]] +
        "-" +
        hex[r[10]] +
        hex[r[11]] +
        hex[r[12]] +
        hex[r[13]] +
        hex[r[14]] +
        hex[r[15]]
      );
    },

    guid(
      a: any // placeholder
    ) {
      return (a // if the placeholder was passed, return
        ? // a random number from 0 to 15
          (
            a ^ // unless b is 8,
            ((Math.random() * // in which case
              16) >> // a random number from
              (a / 4))
          ) // 8 to 11
            .toString(16) // in hexadecimal
        : // or otherwise a concatenated string:
          1e7 + // 10000000 +
          -1e3 + // -1000 +
          -4e3 + // -4000 +
          -8e3 + // -80000000 +
          -1e11 + // -100000000000,
          ""
      ).replace(
        // replacing
        /[018]/g, // zeroes, ones, and eights with
        this.guid // random hex digits
      );
    },
    /**
     * Get a debounced verison of a function with a given wait time.
     * @param func - function to call afted debouncing
     * @param wait - amount of milliseconds to wait before calling "func"
     * @param immediate - forces the function to be called immediatly
     * @returns A debounced version of the given funciton that may be called.
     */
    getDebouncedFunc(func: Function, wait: number, immediate: boolean = false) {
      var timeout: any;
      return () => {
        const context: any = this as any;
        const args = arguments;

        //function to be called after timeout
        let later = () => {
          timeout = null;
          if (!immediate) {
            func.apply(context, args);
          }
        };
        //Condition to exec function immediatly
        let callNow = immediate && !timeout;
        if (callNow) {
          func.apply(context, args);
        }
        // Set new timeout
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
      };
    },
    /**
     * Debounce the call of a function with a given wait time.
     * @param func - function to call afted debouncing
     * @param wait - amount of milliseconds to wait before calling "func"
     * @param immediate - forces the function to be called immediatly
     * @returns The returned value of the given function.
     */
    debouncedFunc(func: Function, wait: number, immediate: boolean = false) {
      var debounceTimeout: any;
      // const context:any = this as any;
      // const args = arguments;
      console.log("Setup debouncing function...");
      //function to be called after timeout
      let later = () => {
        console.log("Calling LATER");
        debounceTimeout = null;
        if (!immediate) {
          return func();
        }
      };
      //Condition to exec function immediatly
      let callNow = immediate && !debounceTimeout;
      if (callNow) {
        return func();
      }
      console.log("Set timeout");
      // Set new timeout
      clearTimeout(debounceTimeout);
      debounceTimeout = setTimeout(later, wait);
    },

    /**
     * Save a byte array buffer to a new blob and download the file.
     * @param fileName Full file name including extension, to be applied to the downloaded file.
     * @param byte Byte array buffer of the file to download.
     * @param extension file extention type to be used by the download.
     */
    saveByteArray(fileName: string, byte: any, extension?: string) {
      // Save byte buffer to new Blob
      let blob = new Blob([byte], { type: "application/" + extension });

      // Create download link
      let link = document.createElement("a");
      link.href = window.URL.createObjectURL(blob);
      // var fileName = fileName;
      link.download = fileName;
      document.body.appendChild(link);

      // Download File from link
      link.click();
      document.body.removeChild(link);
    },
    _base64ToArrayBuffer(base64: any) {
      var binary_string = window.atob(base64);
      var len = binary_string.length;
      var bytes = new Uint8Array(len);
      for (var i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
      }
      return bytes.buffer;
    },
    /**
     * Save byte array to blob and download file.
     * @param {Array} byteArray array of bytes representig the requested file.
     * @param {String} fileName file name to be used when generating file download.
     */
    downloadFileFromBytes(byteArray: any[], fileName: string) {
      if (byteArray.length > 0) {
        // Geneare array buffer
        const bytebuffer: any = this._base64ToArrayBuffer(byteArray);
        // If missing, generate fileName
        if (!fileName) {
          fileName = `generated_file_${new Date()
            .toISOString()
            .split(/[-:.]/)
            .join("")}`;
        }
        //Save to blob and download file
        this.saveByteArray(fileName, bytebuffer);
      }
    },
    async downloadExcelForView(
      tableName: string,
      tableCondition: string,
      fileName: string,
      officeId: number
    ) {
      try {
        // Get column labels
        const tableStructPayload: GenericSqlSelectPayload = {
          viewName: tableName,
        };
        const tableStruct: any = await this.getJRSTableStructure({
          genericSqlPayload: tableStructPayload,
        });
        const cols: Array<JrsHeader> = tableStruct.columns;

        const colList: string = cols
          .filter((header: JrsHeader) => !header.is_hidden)
          .sort((a: JrsHeader, b: JrsHeader) => {
            let order_A: number = a.list_order ? a.list_order : 1000;
            let order_B: number = b.list_order ? b.list_order : 1000;
            return order_A < order_B ? -1 : 1;
          })
          .reduce(
            (retStr: string, col: JrsHeader) => `${retStr},${col.column_name}`,
            ""
          )
          .substring(1);
        const colLabels: Array<GenericExcelColumnLabel> = cols
          .filter((col: JrsHeader) => !col.is_hidden)
          .map((col: JrsHeader) => {
            let ret = {
              columnName: col.column_name,
              columnLabel: col.translationtKey
                ? translate(col.translationtKey).toString()
                : null,
            };
            return ret;
          });

        // Get excel document bytes
        const byteArray: Array<any> = await this.generateExcelForTable({
          viewName: tableName,
          whereCond: tableCondition,
          orderStmt: undefined,
          officeId: officeId,
          colList: colList,
          colLabels: colLabels,
        });

        // Download File
        const dateString: string = new Date().toLocaleDateString("en-US");
        this.downloadFileFromBytes(byteArray, fileName);
      } catch (error) {
        throw error;
        // this.showNewSnackbar({ typeName: "error", text: error });
      }
    },
    /**
     * Get value of cookie.
     * @param cName Name of cookie to get.
     */
    getCookie(cName: string): string {
      const decodedCookie: string = decodeURIComponent(document.cookie);
      const cookieList: string[] = decodedCookie.split(";");
      let cookieVal: string = "";

      for (let c of cookieList) {
        const cParts: string[] = c.split("=");
        if (cParts[0] == cName) {
          cookieVal = cParts[1];
          break;
        }
      }
      return cookieVal;
    },

    async synchInsert(payload: GenericSqlPayload) {
      let localThis = this as any;
      const retvalue = await localThis.execSPCall(payload);
      return retvalue;
    },
    generateError(item: any) {
      let localThis: any = this as any;
      localThis.editMode = false;
      if (item != null) {
        if (typeof item == "string") {
          if (
            item == "Cannot read properties of undefined (reading 'status')"
          ) {
            item = "BROKEN CONNECTION! Refresh the page";
          }
          localThis.showNewSnackbar({
            typeName: "error",
            text: item,
          });
        } else {
          let errMessage = "";
          if (item.message != undefined) {
            if (
              item.message ==
              "Cannot read properties of undefined (reading 'status')"
            ) {
              errMessage = "BROKEN CONNECTION! Refresh the page";
            } else {
              errMessage = item.message;
            }
          }
          localThis.showNewSnackbar({
            typeName: "error",
            text: errMessage,
          });
        }

        // Feedback to user
      } else {
        localThis.showNewSnackbar({
          typeName: "error",
          text: "Error! Refresh the page",
        });
      }
    },

    // format as "DD/MM/YYYY"
    formatDate(date: Date) {
      let localThis: any = this as any;
      return [
        localThis.padTo2Digits(date.getDate()),
        localThis.padTo2Digits(date.getMonth() + 1),
        date.getFullYear(),
      ].join("/");
    },
    // Format using reusable function
    padTo2Digits(num: number) {
      return num.toString().padStart(2, "0");
    },
  },
};
