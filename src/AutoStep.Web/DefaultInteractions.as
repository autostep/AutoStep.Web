
Trait: named

  locateNamed(name): needs-defining

Trait: clickable + named

  Step: Given I have clicked the {name} $component$
    locateNamed(name) -> visible() -> click()

Trait: editable + named

  Step: Given I have entered {text} into the {name} $component$
    locateNamed(name) -> visible() -> type(text)

Component: button

  traits: clickable, named

  locateNamed(name): select('input[type=submit]') -> withAttribute('aria-label', name)

Component: field

  traits: editable, named

  locateNamed(name): select('input[type=text]') -> withAttribute('aria-label', name)

Component: page

  Step: Then the {name} page should be displayed
    getPageTitle() -> assertText(name)

  getPageTitle(): select('title') -> getInnerText()