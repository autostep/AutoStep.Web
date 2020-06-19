export function closestParent(elements, selector)
{
    let results = [];
    
    elements.forEach(el => {
        var currentParent = el.parentElement;
        while (currentParent)
        {
            if (selector)
            {
                if (currentParent.matches(selector))
                {
                    results.push(currentParent);
                    break;
                }
            }
            else 
            {
                results.push(currentParent);
                break;
            } 

            currentParent = currentParent.parentElement;
        }
    });

    return results;
}