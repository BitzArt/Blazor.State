export function getInnerText(id) {

    console.log('getInnerText was called');
    console.log('document', document);

    let element = document.getElementById(id);

    console.log('element', element);

    if (element) {
        return element.innerText;
    }

    return null;
}