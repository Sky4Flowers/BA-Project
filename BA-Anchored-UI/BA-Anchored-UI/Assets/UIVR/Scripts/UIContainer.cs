using UnityEngine;

public interface UIContainer {

    void addElement(AnchoredUI element);

    void addContainer(UIContainer container);

    bool activateElementWithID(int id);

    Vector2 getRelativeSize();

    void resize(float newX, float newY);

    void setupCylinderElements();
}