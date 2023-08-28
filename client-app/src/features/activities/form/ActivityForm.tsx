import { useEffect, useState } from 'react';
import { Button, Header, Segment } from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Activity, ActivityFormValues } from '../../../app/models/activity';
import LoadingComponent from '../../../app/layout/LoadingComponents';
import { Formik ,Form} from 'formik';
import * as Yup from 'yup';
import MyTextInput from '../../../app/common/form/MyTextInput';
import MyTextArea from '../../../app/common/form/MyTextArea';
import MySelectInput from '../../../app/common/form/MySelectInput';
import { categoryOptions } from '../../../app/common/options/categoryOptions';
import MyDatetInput from '../../../app/common/form/MyDateInput';
import { v4 as uuid } from 'uuid';

export default observer(function ActivityForm(){
    const {activityStore}=useStore();
    const {createActivity,updateActivity,loading,loadActivity,loadingInitial} = activityStore;
    const {id} = useParams();
    const navigate = useNavigate();
    const [activity,setActivity] = useState<ActivityFormValues>(new ActivityFormValues());

    const validationSchema = Yup.object({
        title : Yup.string().required('The activity title is required'),
        description:Yup.string().required(),
        category:Yup.string().required(),
        date:Yup.string().required('Date is required'),
        venue:Yup.string().required(),
        city:Yup.string().required()
    })
   
    useEffect(()=>{
        if(id) loadActivity(id).then(activity=>setActivity(new ActivityFormValues(activity)))
    },[id,loadActivity])
    
    function handleFormSubmit(activity:ActivityFormValues){
        if(!activity.id){
            activity.id =uuid();
            createActivity(activity).then(()=>navigate(`/activities/${activity.id}`))
        }
        else{
            updateActivity(activity).then(()=>navigate(`/activities/${activity.id}`))
        }
      
  }
  

    if(loadingInitial) return <LoadingComponent content='Loading activity...'/>;

    return(
        <Segment clearing> 
        <Header content='Activity Details' sub color='teal'></Header>
            <Formik 
            validationSchema={validationSchema}
            enableReinitialize 
            initialValues={activity} 
            onSubmit={values=>handleFormSubmit(values)}>
                {({handleSubmit,isValid,isSubmitting,dirty})=>(
               <Form className='ui form' onSubmit={handleSubmit} >
            
              < MyTextInput name='title' placeholder='Title'/>
          
               <MyTextArea   name='description' placeholder='Description' rows={0}  />
               <MySelectInput placeholder='Category' name='category' options={categoryOptions} />
               <MyDatetInput 
                placeholderText='Date'
                 name='date'
                 showTimeSelect
                 timeCaption='time'
                 dateFormat='MMMM d,yyyy h:mm aa' />
                   <Header content='Location Details' sub color='teal'></Header>
               <MyTextInput placeholder='City' name='city' />
               <MyTextInput placeholder='Venue'  name='venue' />
               <Button 
               disabled={isSubmitting || !dirty || !isValid}
               loading={isSubmitting} floated='right' positive type='submit' content='submit'   name='title' />
               <Button as={Link}  to='/activities' floated='right' type='button' content='cancel'  name='title' />

      </Form>

                )}
            </Formik>
      
            
        </Segment>
    )
})