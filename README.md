# AutoStep.Web
Web Application Testing Behaviour for AutoStep

Example Steps:

This step is from the 'clickable' trait, which has a generated step for the 'menu' control

  Given I have clicked the menu

This step is from the 'editable' trait, which has a generated step for the built-in input control.
The 'Name' value gets passed to the 'Labeled' trait, which can output a defined control entity (which then has the editable trait).

  Given I have entered 'Value' into 'Name'

Traits can have different configuration for the whole app, or even a specific sub-section of the application (maybe).
Some sort of trait scoping

So, a step for a particular trait can invoke behaviour from another trait if required.
or the step above, the pseudo-code looks like this:

  EnterValueStep
   -> Locate Control (go through control locators for 'Labeled' trait)
   -> Determine Control Type
   -> Editable Trait (for appropriate control type)

Control Types have locators:
 menu
 input
 text-input

Traits must be written in C#?

```

{
  "controls": {
    "input": {
      matches: "input",
      traits: ["positioned"]
    },
    "text": {
      matches: "input[type=text]",
      traits: ["editable", "clickable"],      
    },
    "dropdown": {
      
    }
  },
  "locators": {
    "labeled": {
      
    }
  }
}

Control: input
  matches: input
  traits: positioned

Control: text
  based-on: input
  matches: input[type=text]
  traits: editable, clickable

// This step is from the labeled+editable trait
Given I have entered {value} into {field}
  - input:find <field>
  - editable:enter <value>

// This step is from the menu trait
Given I have clicked the menu
  -> menu:find  
  -> click

When I press {button}
 - button:find
 - 
```

## Training

Define new control types
Configure existing control types

Defining a new control:
  - Tell a locator how to find a control
  - Work out probable traits
  - Allow additional traits to be specified
  - Customise trait actions (select item means 'blah blah')

// Perhaps traits are defined in code? Saying a control implements a trait is basically it implementing an interface.

# Definition

## Define App Context
App: my-app

  name: 'My App'

Trait: clickable + named + enable-disable
  
  Step: Given I have clicked the {name} $component$
    This step will click the $component$.

    -> locateNamed(<name>)
    -> first()
    -> assertEnabled()
    -> click()

Trait: enable-disable
  
  assertEnabled(): isEnabled() -> assertTrue()
  assertDisabled(): isEnabled() -> assertFalse()

Trait: named

  default: locateNamed(<name>)

  locateNamed: undefined()

Trait: named + enable-disable

  Step: Then the {name} $component$ should be enabled
    locateNamed(<name>)
    -> first()
    -> assertEnabled()

  Step: Then the {name} $component$ should be disabled
    locateNamed(name)
    -> first()
    -> assertEnabled()

# The singular trait implies there is only one 
# on the page.
Trait: singular

Trait: has-value

  getValue: getAttribute('value')


Trait: has-value + named

  Step: Then {name} should have the value {value}
    locateNamed(<name>)
    -> getValue()
    
Trait: named + displayable
  
  Step: Then the {name} $component$ should exist
    locateNamed(<name>)
    -> assertOneExists()

  Step: Then the {name} $component$ should not exist
    locateNamed(<name>)
    -> assertNoneExist()


  Step: Then the {name} $component$ should be visible
    locatedNamed(<name>)
    -> assertVisible()

Trait: scannable + named

  Step: Given I have scanned {value:list} into {name}
    locateNamed(<name>)
    -> forEachItemIn(<value>)
    -> type(<item>)
    -> typeEscape()

  Step: Given I have scanned the following values into {name}
    locateNamed(<name>)
    -> forEachTableRow()
    -> type(<row:value>)
    -> typeEscape()

Trait: scannable-list + named

  selectListItems(): select('li')

  Step: Then {value} should be in the {name} $component$
    locateNamed(<name>)
    -> selectListItems()
    -> getText()
    -> assertOneHasText(<value>)

Trait: editable + named

  isReadOnly(): -> getAttribute('readonly') -> equals('readonly')

  assertReadOnly(): isReadOnly() -> assertTrue()
  assertNotReadOnly(): isReadOnly() -> assertFalse()

  Step: When I press Tab in {name}
    locateNamed(<name>)
    -> type(TAB)
    
  Step: When I press Escape in {name}
    locateNamed(<name>)
    -> type(TAB)

  Step: Then the {name} control should be readonly
    locateNamed()
    -> assertReadOnly()

Trait: selectable + named
 
  Step: Then the {name} $component$ should have {count} items and contain the values {values:list}
    -> locateNamed(name)
    -> getItems()
    -> assertCountIs(count)
    -> assertContainsAll(values)

Trait: checkable + named

  Step: Given I have checked the {name} checkbox
    -> locateNamed(<name>)
    -> check()

  Step: Given I have un-checked the {name} checkbox
    -> locateNamed(<name>)
    -> uncheck()
    
# Base control component
Component: control

  traits: displayable

# Defining a button
Component: button
  Description

  traits: clickable, enable-disable, named
  locateNamed(name): select('input[type=button], button') -> with-text(<name>)
  based-on: control
      

# Example of overriding locate for a default control
Component: button
  This component is the override for buttons
  
  locateNamed(name): select('input.bootstrap-btn') -> with-text(<name>)
  based-on: button

Component: kendo-button

  name: 'kendo button'

# Defining the 'field'
Component: field

  traits: clickable, enable-disable, named, scannable, has-value
  locateAll(): select('input[type=text]')
  locateNamed(name): label(<name>) -> linkedComponent()
 
# Page Example
Component: page

  Step: Then the {title} page should be displayed
    -> select('head title') -> get-text() -> assertEqualTo(<title>)

  Step: Then a page with {partialTitle} in the title should be displayed
    -> select('head title')
    -> get-text()
    -> assert-contains(<title>)

  Step: Given I press Tab
    -> type(TAB)
    
  Step: Given I press Escape
    -> type(ESCAPE)

  Step: Given I press Tab {count} times
    -> repeat(<count>)
    -> wait(0.5)

Component: kendo-grid
  
  name: 'kendo grid'
 
  locate(): -> select('k-grid') -> displayed()
  locateNamed(name): -> label(name) -> linkedComponent() -> withClass('k-grid') -> displayed()
  
  getGridRow: -> select('k-grid')

  Step: Given I have clicked the {linkName} in the kendo grid where {column} equals {columnValue}
    -> locate()
    -> getRowByColumn({column}, {columnValue}) # Defined in code, because it can't be done here
    -> link(<linkName>)
    -> click()
    
  Step: Then the {linkName} link column in the kendo grid should be shown
    -> locate()    
    -> link(<linkName>)
    -> assertAtLeastOne()

  Step: Then the {linkName} in the kendo grid where {column} equals {columnValue} is disabled
    -> locate()
    -> getRowByColumn(<column>, <columnValue>)
    -> link(<linkName>)
    -> assertEnabled()

  Step: Then the kendo grid should have {count:int} rows
    -> locate()
    -> remember('grid') // Stores the grid as a local variable
    -> waitForNo('k-loading-mask')
    -> select('tr[role=row]')
    -> assertCountIs(<count>)
        
  Step: Then the kendo grid should have at least {count:int} rows
    -> locate()
    -> remember('grid') // Stores the grid as a local variable
    -> waitForNo('k-loading-mask')
    -> select('tr[role=row]')
    -> assertCountIsAtLeast(<count>)
    

Component: label
  
  traits: named

  locateNamed()

Component: heading

  locateNamed: select('h1,h2,h3') -> with-text(<name>)

  traits: named, displayable

Component: link
  
  traits: named, displayable, clickable, enable-disable

  locateNamed: -> select('a') -> with-text(<name>)
