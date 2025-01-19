export function getInnerText(id) {

    console.debug('getInnerText was called');
    console.debug('document', document);

    let element = document.getElementById(id);

    console.debug('element', element);

    if (element) {
        return element.innerText;
    }

    return null;
}