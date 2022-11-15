// create saveAsFile function that get the file name and the file content and data
function saveAsFile(fileName, fileContent, data) {
    var blob = new Blob([fileContent], { type: "text/plain;charset=utf-8" });

    //add data
    var data = data;
    
    saveAs(blob, fileName);
}