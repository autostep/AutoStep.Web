export function clearInputs(elements)
{
    elements.forEach(element => {
        element.value = "";
    });
}