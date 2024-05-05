using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private GameObject _anchor;
    private GameObject _exit;
    private Animator _animator;
    private StoryManager _storyManager;
    // Start is called before the first frame update
    void Awake()
    {
        _storyManager = GameObject.Find("StoryManager").GetComponent<StoryManager>();
        _anchor = GameObject.Find("Mouse Movement");
        _animator = GameObject.Find("Rat").GetComponent<Animator>();
        _animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_storyManager.GetIsInDialogue())
        {
            _anchor.transform.position = transform.position;
        }
        var distance = Vector3.Distance(this.transform.position, _anchor.transform.position);
        if (_anchor != null)
        {
            if (distance > 0.01f)
            {
                _animator.enabled = true;
                LookAtYaxis(gameObject, _anchor.transform.position);
                // graffelo follow the mouse
                if (Vector3.Distance(transform.position, _anchor.transform.position) > 0.01f)
                {
                    Vector3 direction = Vector3.Normalize(_anchor.transform.position - transform.position);

                    transform.position = Vector3.MoveTowards(transform.position, _anchor.transform.position, 0.08f * Time.deltaTime);
                }
            }
            else
            {
                _animator.enabled = false;
            }
        }
    }


    private void LookAtYaxis(GameObject self, Vector3 target)
    {
        Vector3 direction = Vector3.Normalize(target - self.transform.position);
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        self.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
    private void OnDisable()
    {
        this.enabled = true;
    }
}
